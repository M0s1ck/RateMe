package usecase

import (
	"S3Service/internal/domain"
	"github.com/gin-gonic/gin"
	"io"
)

type PhotoUseCase struct {
	repo domain.PhotoRepository
}

func NewPhotoUseCase(repo domain.PhotoRepository) *PhotoUseCase {
	return &PhotoUseCase{repo: repo}
}

func (uc *PhotoUseCase) Get(id string) (io.ReadCloser, int64, error) {
	return uc.repo.Get(id)
}

func (uc *PhotoUseCase) UploadPhoto(c *gin.Context) {}
