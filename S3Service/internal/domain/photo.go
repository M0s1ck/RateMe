package domain

import (
	"io"
	"mime/multipart"
	"net/url"
)

type PhotoRepository interface {
	Get(id string) (io.ReadCloser, int64, error)
	Upload(multipart.FileHeader) (string, error)
}

type PhotoPresignedRepo interface {
	Get(id string) (presigned *url.URL, err error)
	Upload(id string) (presigned *url.URL, err error)
	Update(id string) (presigned *url.URL, err error)
	Remove(id string) (err error)
}
