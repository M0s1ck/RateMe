package repository

import (
	"io"
	"mime/multipart"
)

var PhotosBucketName string = "photos"

type PhotoMinioRepo struct {
	fileRepo *FileRepo
}

func NewPhotoMinioRepo(fileRepo *FileRepo) *PhotoMinioRepo {
	return &PhotoMinioRepo{fileRepo: fileRepo}
}

func (repo *PhotoMinioRepo) Get(id string) (io.ReadCloser, int64, error) {
	return repo.fileRepo.Get(id, PhotosBucketName)
}

func (repo *PhotoMinioRepo) Upload(file multipart.FileHeader) (string, error) {
	return "", nil
}
