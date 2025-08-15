package delivery

import (
	"S3Service/internal/domain"
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
	engine.GET("presigned/get/:id", ph.GetPresigned)
	engine.GET("presigned/upload", ph.UploadPresigned)
}

func (ph *PhotoHandler) GetHello(c *gin.Context) {
	c.IndentedJSON(200, gin.H{"text": "Hello, world!"})
}

// Get godoc
//
//	@Summary		Get a photo
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
//	@Success		200	{object}	map[string]string
//	@Failure		404	{object}	map[string]string
//	@Failure		500	{object}	map[string]string
//	@Router			/presigned/get/{id} [get]
func (ph *PhotoHandler) GetPresigned(c *gin.Context) {
	id := c.Param("id")
	url, err := ph.photoUC.GetPresigned(id)

	if errors.Is(err, domain.ErrNotFound) {
		c.JSON(http.StatusNotFound, gin.H{"message": fmt.Sprintf("Photo with id=%v was not found", id)})
		return
	}

	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"message": err.Error()})
		return
	}

	c.IndentedJSON(200, gin.H{"url": url.String()})
}

// UploadPresigned godoc
//
//	@Summary		Get a presigned url to upload a photo
//	@Description	Gets a presigned url to upload a photo to MinIO storage
//	@Tags			Photos
//	@Accept			mpfd
//	@Produce		json
//	@Success		200	{object}	map[string]string
//	@Failure		500	{object}	map[string]string
//	@Router			/presigned/upload [get]
func (ph *PhotoHandler) UploadPresigned(c *gin.Context) {
	url, err := ph.photoUC.UploadPresigned()

	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"message": err.Error()})
		return
	}

	c.IndentedJSON(200, gin.H{"url": url.String()})
}
