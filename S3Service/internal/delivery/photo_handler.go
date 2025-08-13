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
}

func (ph *PhotoHandler) GetHello(c *gin.Context) {
	c.IndentedJSON(200, gin.H{"text": "Hello, world!"})
}

func (ph *PhotoHandler) Get(c *gin.Context) { // TODO: Test
	id := c.Param("id")
	reader, size, err := ph.photoUC.Get(id)

	c.Header("Content-Disposition", fmt.Sprintf("attachment; filename=\"%s\"", id))
	c.DataFromReader(http.StatusOK, size, "application/octet-stream", reader, nil)

	_ = reader.Close()

	if err != nil { // TODO: if errors.Is(err, domain.ErrNotFound)
		c.JSON(http.StatusNotFound, gin.H{"message": err.Error()})
		return
	}
}

func (ph *PhotoHandler) Upload(c *gin.Context) {
	c.IndentedJSON(200, gin.H{"text": "Hello, world!"})
}
