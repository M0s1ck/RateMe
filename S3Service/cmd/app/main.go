package main

import (
	"S3Service/internal/delivery"
	"fmt"
	"github.com/gin-gonic/gin"
)

func main() {
	engine := gin.Default()
	photoHandler := delivery.PhotoHandler{}
	photoHandler.RegisterRoutes(engine)

	err := engine.Run(":8080")

	if err != nil {
		fmt.Println(err)
		panic(err)
	}
}
