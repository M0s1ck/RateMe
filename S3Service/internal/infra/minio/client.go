package minio

import (
	"github.com/minio/minio-go/v7"
	"github.com/minio/minio-go/v7/pkg/credentials"
	"log"
)

func NewMinioClient() *minio.Client {
	//TODO: fix!! Launches in docker bu gives presigned urls with "minio:9000" domain
	endpoint := "minio:9000" // TODO: вынести в .env, config for dev and prod (localhost for local and minio for docker)
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
