package main

import (
	"S3Service/api"
	"S3Service/internal/delivery"
	inframinio "S3Service/internal/infra/minio"
	"S3Service/internal/repository"
	"S3Service/internal/usecase"
	"fmt"
	"github.com/gin-gonic/gin"
	"github.com/joho/godotenv"
	"github.com/minio/minio-go/v7"
	swaggerfiles "github.com/swaggo/files"
	ginSwagger "github.com/swaggo/gin-swagger"
	"log"
	"os"
	"path/filepath"
	"runtime"
)

const photosBucketName = "photos"
const envPortKey = "S3_SERVICE_PORT"

func main() {
	loadEnv()

	// Set up minio
	var minioClient *minio.Client = inframinio.NewMinioClient()
	inframinio.SetupBucket(minioClient, photosBucketName)

	// DI
	minioFileRepo := repository.NewFileRepo(minioClient)
	photoRepo := repository.NewPhotoMinioRepo(minioFileRepo)

	s3PresignedRepo := repository.NewS3PresignedRepo(minioClient)
	photoPresignedRepo := repository.NewPhotoPresignedRepo(s3PresignedRepo)

	photoUseCase := usecase.NewPhotoUseCase(photoRepo, photoPresignedRepo)
	photoHandler := delivery.NewPhotoHandler(photoUseCase)

	// Gin app
	engine := gin.Default()

	photoHandler.RegisterRoutes(engine)

	api.SwaggerInfo.BasePath = ""
	engine.GET("/swagger/*any", ginSwagger.WrapHandler(swaggerfiles.Handler))

	addr := ":" + os.Getenv(envPortKey)
	err := engine.Run(addr)

	if err != nil {
		fmt.Println(err)
		panic(err)
	}
}

func loadEnv() {
	_, filename, _, _ := runtime.Caller(0)
	dir := filepath.Join(filepath.Dir(filename), "../../..")

	err := godotenv.Load(filepath.Join(dir, ".env"))
	if err != nil {
		log.Println(".env file not found. Ignored")
	}

	// Check
	_, exists := os.LookupEnv(envPortKey)
	if !exists {
		log.Fatalln("Port variable not found in env. Check if env is loaded")
	}
}
