package delivery

import (
	"S3Service/internal/usecase"
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

	if err != nil { // TODO: if errors.Is(err, domain.ErrNotFound)
		c.JSON(http.StatusNotFound, gin.H{"message": err.Error()})
		return
	}

	c.Header("Content-Disposition", fmt.Sprintf("attachment; filename=\"%s\"", id))
	c.DataFromReader(http.StatusOK, size, "application/octet-stream", reader, nil)
	_ = reader.Close()
}

// Upload godoc
//
//	@Summary		Upload a photo
//	@Description	Upload a user photo to MinIO storage
//	@Tags			Photos
//	@Accept			mpfd
//	@Produce		json
//	@Param			file	formData	file	true	"Photo file"
//	@Success		200	{object}	map[string]string
//	@Failure		400	{object}	map[string]string
//	@Failure		500	{object}	map[string]string
//	@Router			/upload [post]
func (ph *PhotoHandler) Upload(c *gin.Context) {
	c.IndentedJSON(200, gin.H{"id": "HelloWorld1234"})
}
