package repository

import (
	"S3Service/internal/domain"
	"context"
	"errors"
	"fmt"
	"github.com/minio/minio-go/v7"
	"io"
	"log"
	"mime/multipart"
)

type FileRepo struct {
	minioClient *minio.Client
}

func NewFileRepo(minioClient *minio.Client) *FileRepo {
	return &FileRepo{minioClient: minioClient}
}

func (repo *FileRepo) Get(name string, bucket string) (io.ReadCloser, int64, error) {
	ctx := context.Background()
	obj, err := repo.minioClient.GetObject(ctx, bucket, name, minio.GetObjectOptions{})

	if err != nil {
		err = fmt.Errorf("minio get object error: %w", err)
		log.Println(err)
		return nil, 0, err
	}

	info, err := obj.Stat()

	if err == nil {
		return obj, info.Size, nil
	}

	err = fmt.Errorf("minio check object error: %w", err)
	log.Println(err)
	var minioErr minio.ErrorResponse

	if errors.As(err, &minioErr) && minioErr.StatusCode == 404 {
		return nil, info.Size, domain.ErrNotFound
	}

	return nil, 0, err
}

func (repo *FileRepo) Upload(file multipart.FileHeader, bucket string) (string, error) {
	return "", nil
}
