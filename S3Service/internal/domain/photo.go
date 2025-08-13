package domain

import (
	"io"
	"mime/multipart"
)

type Photo struct {
	Name string
}

func NewPhoto(name string) *Photo {
	return &Photo{Name: name}
}

type PhotoRepository interface {
	Get(id string) (io.ReadCloser, int64, error)
	Upload(multipart.FileHeader) (string, error)
}
