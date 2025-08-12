package delivery

import "github.com/gin-gonic/gin"

type PhotoHandler struct {
}

func (ph *PhotoHandler) RegisterRoutes(engine *gin.Engine) {
	engine.GET("", ph.GetHello)
}

func (ph *PhotoHandler) GetHello(c *gin.Context) {
	c.IndentedJSON(200, gin.H{"text": "Hello, world!"})
}
