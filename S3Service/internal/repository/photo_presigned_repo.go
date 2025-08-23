package repository

import (
	"S3Service/internal/domain"
	"net/url"
)

var photoBucketName = "photos"
var photoExtension = "jpg"

type PhotoPresignedRepo struct {
	s3Repo *S3PresignedRepo
}

func NewPhotoPresignedRepo(s3Repo *S3PresignedRepo) *PhotoPresignedRepo {
	return &PhotoPresignedRepo{s3Repo: s3Repo}
}

func (repo *PhotoPresignedRepo) Get(id string) (url *url.URL, err error) {
	objName := id + "." + photoExtension
	exists, err := repo.s3Repo.CheckIfExists(objName, photoBucketName)

	if err != nil {
		return nil, err
	}

	if !exists {
		return nil, domain.ErrNotFound
	}

	return repo.s3Repo.Get(objName, photoBucketName)
}

func (repo *PhotoPresignedRepo) Upload(id string) (presigned *url.URL, err error) {
	objName := id + "." + photoExtension
	return repo.s3Repo.Upload(objName, photoBucketName)
}

func (repo *PhotoPresignedRepo) Update(id string) (presigned *url.URL, err error) {
	objName := id + "." + photoExtension
	exists, err := repo.s3Repo.CheckIfExists(objName, photoBucketName)

	if err != nil {
		return nil, err
	}

	if !exists {
		return nil, domain.ErrNotFound
	}

	return repo.s3Repo.Upload(objName, photoBucketName)
}

func (repo *PhotoPresignedRepo) Remove(id string) (err error) {
	objName := id + "." + photoExtension
	return repo.s3Repo.Remove(objName, photoBucketName)
}
