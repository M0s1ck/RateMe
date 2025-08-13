package main

import (
	"S3Service/internal/delivery"
	infraMinio "S3Service/internal/infra/minio"
	"S3Service/internal/repository"
	"S3Service/internal/usecase"
	"fmt"
	"github.com/gin-gonic/gin"
	"github.com/minio/minio-go/v7"
)

func main() {
	// Set up minio
	var minioClient *minio.Client = infraMinio.NewMinioClient()
	infraMinio.SetupBucket(minioClient, "photos")

	// DI
	minioFileRepo := repository.NewFileRepo(minioClient)
	photoRepo := repository.NewPhotoMinioRepo(minioFileRepo)
	photoUseCase := usecase.NewPhotoUseCase(photoRepo)
	photoHandler := delivery.NewPhotoHandler(photoUseCase)

	// Gin app
	engine := gin.Default()

	photoHandler.RegisterRoutes(engine)

	err := engine.Run(":8080")

	if err != nil {
		fmt.Println(err)
		panic(err)
	}
}
