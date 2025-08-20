package minio

import (
	"github.com/minio/minio-go/v7"
	"github.com/minio/minio-go/v7/pkg/credentials"
	"log"
	"os"
)

func NewMinioClient() *minio.Client {
	// Now works only via etc/hosts
	// TODO:Potentially add nginx support

	hostName := os.Getenv("MINIO_HOST_ALIAS")
	port := os.Getenv("MINIO_S3_API_PORT")
	endpoint := hostName + ":" + port

	accessKeyID := os.Getenv("MINIO_ROOT_USER")
	secretAccessKey := os.Getenv("MINIO_ROOT_PASSWORD")

	minioClient, err := minio.New(endpoint, &minio.Options{
		Creds:  credentials.NewStaticV4(accessKeyID, secretAccessKey, ""),
		Secure: false,
	})

	if err != nil {
		panic(err)
	}

	log.Println("Minio client is set up")
	return minioClient
}
