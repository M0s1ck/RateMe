package delivery

import (
	"github.com/gin-gonic/gin"
	"net/http"
)

// Health godoc
//
//	@Summary		Service healthcheck
//	@Tags			Health
//	@Success		200    "OK"
//	@Router			/health [get]
func Health(c *gin.Context) {
	c.Status(http.StatusOK)
}
