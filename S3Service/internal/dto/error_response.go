package dto

type ErrorNotFoundResponse struct {
	Message string `json:"message" example:"Photo with id=1214a280-1162-408a-918f-5cb9300194ce was not found"`
}

type ErrorInternalResponse struct {
	Message string `json:"message" example:"Internal Server Error"`
}
