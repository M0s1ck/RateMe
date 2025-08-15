package repository

import (
	"S3Service/internal/domain"
	"context"
	"fmt"
	"github.com/google/uuid"
	"github.com/minio/minio-go/v7"
	"net/url"
	"time"
)

type S3PresignedRepo struct {
	minioClient *minio.Client
}

func NewS3PresignedRepo(minioClient *minio.Client) *S3PresignedRepo {
	return &S3PresignedRepo{minioClient: minioClient}
}

func (repo *S3PresignedRepo) Get(name string, bucket string, extension string) (*url.URL, error) {
	ctx := context.Background()
	objName := name + "." + extension

	// Check if exists
	_, err := repo.minioClient.StatObject(ctx, bucket, objName, minio.StatObjectOptions{})

	if err != nil {
		if minio.ToErrorResponse(err).StatusCode == 404 {
			return nil, domain.ErrNotFound
		}
		return nil, err
	}

	reqParams := make(url.Values)
	reqParams.Set("response-content-disposition", fmt.Sprintf("attachment; filename=\"%v\"", objName))

	presignedURL, err := repo.minioClient.PresignedGetObject(ctx, bucket, objName, time.Second*1000, reqParams)

	if err != nil {
		return nil, err
	}

	return presignedURL, err
}

func (repo *S3PresignedRepo) Upload(bucket string, extension string) (presigned *url.URL, err error) {
	ctx := context.Background()

	id := uuid.NewString()
	objName := id + "." + extension

	presignedURL, err := repo.minioClient.PresignedPutObject(ctx, bucket, objName, time.Second*1000)

	if err != nil {
		return nil, err
	}

	return presignedURL, err
}
