package repository

import (
	"context"
	"fmt"
	"github.com/minio/minio-go/v7"
	"log"
	"net/url"
	"time"
)

const notFoundStatusCode int = 404

type S3PresignedRepo struct {
	minioClient *minio.Client
}

func NewS3PresignedRepo(minioClient *minio.Client) *S3PresignedRepo {
	return &S3PresignedRepo{minioClient: minioClient}
}

func (repo *S3PresignedRepo) Get(objName string, bucket string) (*url.URL, error) {
	ctx := context.Background()

	reqParams := make(url.Values)
	reqParams.Set("response-content-disposition", fmt.Sprintf("attachment; filename=\"%v\"", objName))

	presignedURL, err := repo.minioClient.PresignedGetObject(ctx, bucket, objName, time.Second*1000, reqParams)

	if err != nil {
		log.Printf("Error presignedGetObject: %v", err)
		return nil, err
	}

	return presignedURL, err
}

func (repo *S3PresignedRepo) Upload(objName string, bucket string) (presigned *url.URL, err error) {
	ctx := context.Background()
	presignedURL, err := repo.minioClient.PresignedPutObject(ctx, bucket, objName, time.Second*1000)

	if err != nil {
		log.Printf("Error presignedPutObject: %v", err)
		return nil, err
	}

	return presignedURL, err
}

func (repo *S3PresignedRepo) CheckIfExists(objName string, bucket string) (res bool, err error) {
	ctx := context.Background()
	_, err = repo.minioClient.StatObject(ctx, bucket, objName, minio.StatObjectOptions{})

	if err != nil {
		if minio.ToErrorResponse(err).StatusCode == notFoundStatusCode {
			return false, nil
		}

		log.Printf("Error statObject: %v", err)
		return true, err
	}

	return true, nil
}

func (repo *S3PresignedRepo) Remove(objName string, bucket string) (err error) {
	ctx := context.Background()
	err = repo.minioClient.RemoveObject(ctx, bucket, objName, minio.RemoveObjectOptions{})

	if err != nil {
		log.Printf("Error removeObject: %v", err)
	}

	return err
}
