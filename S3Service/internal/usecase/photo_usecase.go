package usecase

import (
	"S3Service/internal/domain"
	"github.com/gin-gonic/gin"
	"io"
	"net/url"
)

type PhotoUseCase struct {
	repo          domain.PhotoRepository
	presignedRepo domain.PhotoPresignedRepo
}

func NewPhotoUseCase(repo domain.PhotoRepository, presignedRepo domain.PhotoPresignedRepo) *PhotoUseCase {
	return &PhotoUseCase{
		repo:          repo,
		presignedRepo: presignedRepo,
	}
}

func (uc *PhotoUseCase) Get(id string) (io.ReadCloser, int64, error) {
	return uc.repo.Get(id)
}

func (uc *PhotoUseCase) GetPresigned(id string) (url *url.URL, err error) {
	return uc.presignedRepo.Get(id)
}

func (uc *PhotoUseCase) UploadPhoto(c *gin.Context) {}
