package main

import (
	"S3Service/api"
	"S3Service/internal/delivery"
	infra "S3Service/internal/infra/minio"
	"S3Service/internal/repository"
	"S3Service/internal/usecase"
	"fmt"
	"github.com/gin-gonic/gin"
	"github.com/minio/minio-go/v7"
	swaggerfiles "github.com/swaggo/files"
	ginSwagger "github.com/swaggo/gin-swagger"
)

func main() {
	// Set up minio
	var minioClient *minio.Client = infra.NewMinioClient()
	infra.SetupBucket(minioClient, "photos")

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

	err := engine.Run(":8080")

	if err != nil {
		fmt.Println(err)
		panic(err)
	}
}
