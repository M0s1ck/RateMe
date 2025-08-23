package minio

import (
	"context"
	"github.com/minio/minio-go/v7"
	"log"
)

func SetupBucket(minioClient *minio.Client, bucketName string) {
	location := "us-east-1" // murica
	ctx := context.Background()

	err := minioClient.MakeBucket(ctx, bucketName, minio.MakeBucketOptions{Region: location})

	if err != nil {
		exists, errBucketExists := minioClient.BucketExists(ctx, bucketName)

		if errBucketExists == nil && exists {
			log.Printf("We already own %s\n", bucketName)
		} else {
			log.Fatalln(err)
		}
	} else {
		log.Printf("Successfully created %s\n", bucketName)
	}
}
