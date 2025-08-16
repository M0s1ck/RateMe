package delivery

import (
	"S3Service/internal/domain"
	"S3Service/internal/dto"
	"S3Service/internal/usecase"
	"errors"
	"fmt"
	"github.com/gin-gonic/gin"
	"net/http"
)

type PhotoHandler struct {
	photoUC *usecase.PhotoUseCase
}

func NewPhotoHandler(photoUc *usecase.PhotoUseCase) *PhotoHandler {
	return &PhotoHandler{photoUC: photoUc}
}

func (ph *PhotoHandler) RegisterRoutes(engine *gin.Engine) {
	engine.GET("", ph.GetHello)
	engine.GET("get/:id", ph.Get)
	engine.GET("presigned/:id", ph.GetPresigned)
	engine.GET("presigned/upload", ph.UploadPresigned)
	engine.PUT("presigned/upload/:id", ph.UpdatePresigned)
	engine.DELETE("remove/:id", ph.Remove)
}

func (ph *PhotoHandler) GetHello(c *gin.Context) {
	c.IndentedJSON(200, gin.H{"text": "Hello, world!"})
}

// Get godoc
//
//	@Summary		Get a photo (file)
//	@Description	Get a file of a photo from storage by id
//	@Tags			Photos
//	@Accept			json
//	@Produce		octet-stream
//	@Param			id	path		string	true	"Photo id"
//	@Success		200	{file}	file
//	@Failure		400	{object}	map[string]string
//	@Failure		500	{object}	map[string]string
//	@Router			/get/{id} [get]
func (ph *PhotoHandler) Get(c *gin.Context) {
	id := c.Param("id")
	reader, size, err := ph.photoUC.Get(id)

	if errors.Is(err, domain.ErrNotFound) {
		c.JSON(http.StatusNotFound, gin.H{"message": fmt.Sprintf("Photo with id=%v was not found", id)})
		return
	}

	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"message": err.Error()})
		return
	}

	c.Header("Content-Disposition", fmt.Sprintf("attachment; filename=\"%s\"", id))
	c.DataFromReader(http.StatusOK, size, "application/octet-stream", reader, nil)
	_ = reader.Close()
}

// GetPresigned godoc
//
//	@Summary		Get a presigned url to a photo
//	@Description	Get a presigned url to a photo from storage by id
//	@Tags			Photos
//	@Accept			json
//	@Produce		json
//	@Param			id	path		string	true	"Photo id"
//	@Success		200	{object}    dto.PresignedUrlResponse
//	@Failure		404	{object}	dto.ErrorNotFoundResponse
//	@Failure		500	{object}	dto.ErrorInternalResponse
//	@Router			/presigned/{id} [get]
func (ph *PhotoHandler) GetPresigned(c *gin.Context) {
	id := c.Param("id")
	url, err := ph.photoUC.GetPresigned(id)

	if errors.Is(err, domain.ErrNotFound) {
		c.JSON(http.StatusNotFound, dto.ErrorNotFoundResponse{Message: fmt.Sprintf("Photo with id=%v was not found", id)})
		return
	}

	if err != nil {
		c.JSON(http.StatusInternalServerError, dto.ErrorInternalResponse{Message: err.Error()})
		return
	}

	c.IndentedJSON(http.StatusOK, dto.PresignedUrlResponse{URL: url.String()})
}

// UploadPresigned godoc
//
//	@Summary		Get a presigned url to upload a photo
//	@Description	Gets a presigned url to upload a new photo to S3
//	@Tags			Photos
//	@Accept			mpfd
//	@Produce		json
//	@Success		200	{object}	dto.PresignedUploadUrlResponse
//	@Failure		500	{object}	dto.ErrorInternalResponse
//	@Router			/presigned/upload [get]
func (ph *PhotoHandler) UploadPresigned(c *gin.Context) {
	url, id, err := ph.photoUC.UploadPresigned()

	if err != nil {
		c.JSON(http.StatusInternalServerError, dto.ErrorInternalResponse{Message: err.Error()})
		return
	}

	c.IndentedJSON(http.StatusOK, dto.PresignedUploadUrlResponse{URL: url.String(), Id: id})
}

// UpdatePresigned godoc
//
//	@Summary		Get a presigned url to update a photo
//	@Description	Get a presigned url to update an existing photo in S3
//	@Tags			Photos
//	@Accept			json
//	@Produce		json
//	@Param			id	path		string	true	"Photo id"
//	@Success		200	{object}    dto.PresignedUploadUrlResponse
//	@Failure		404	{object}	dto.ErrorNotFoundResponse
//	@Failure		500	{object}	dto.ErrorInternalResponse
//	@Router			/presigned/upload/{id} [put]
func (ph *PhotoHandler) UpdatePresigned(c *gin.Context) {
	id := c.Param("id")
	url, err := ph.photoUC.UpdatePresigned(id)

	if errors.Is(err, domain.ErrNotFound) {
		c.JSON(http.StatusNotFound, dto.ErrorNotFoundResponse{Message: fmt.Sprintf("Photo with id=%v was not found", id)})
		return
	}

	if err != nil {
		c.JSON(http.StatusInternalServerError, dto.ErrorInternalResponse{Message: err.Error()})
		return
	}

	c.IndentedJSON(http.StatusOK, dto.PresignedUrlResponse{URL: url.String()})
}

// Remove godoc
//
//	@Summary		Remove a photo
//	@Description	Remove a photo from S3 by id
//	@Tags			Photos
//	@Accept			json
//	@Produce		json
//	@Param			id	path		string	true	"Photo id"
//	@Success		204    "No content"
//	@Failure		500	{object}	dto.ErrorInternalResponse
//	@Router			/remove/{id} [delete]
func (ph *PhotoHandler) Remove(c *gin.Context) {
	id := c.Param("id")
	err := ph.photoUC.Remove(id)

	if err != nil {
		c.JSON(http.StatusInternalServerError, dto.ErrorInternalResponse{Message: err.Error()})
		return
	}

	c.Status(http.StatusNoContent)
}
