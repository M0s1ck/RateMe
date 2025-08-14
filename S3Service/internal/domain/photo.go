package domain

import (
	"io"
	"mime/multipart"
)

type PhotoRepository interface {
	Get(id string) (io.ReadCloser, int64, error)
	Upload(multipart.FileHeader) (string, error)
}
