package repository

import (
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
	return repo.s3Repo.Get(id, photoBucketName, photoExtension)
}

func (repo *PhotoPresignedRepo) Upload() (presigned *url.URL, err error) {
	panic("implement me")
}
