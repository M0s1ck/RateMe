package minio

import (
	"github.com/minio/minio-go/v7"
	"github.com/minio/minio-go/v7/pkg/credentials"
	"log"
)

func NewMinioClient() *minio.Client {
	endpoint := "localhost:9000" // TODO: вынести в .env
	accessKeyID := "minioadmin"
	secretAccessKey := "minioadmin"

	minioClient, err := minio.New(endpoint, &minio.Options{
		Creds:  credentials.NewStaticV4(accessKeyID, secretAccessKey, ""),
		Secure: false,
	})

	if err != nil {
		panic(err)
	}

	log.Printf("%#v\n", minioClient) // minioClient is now set up

	return minioClient
}
