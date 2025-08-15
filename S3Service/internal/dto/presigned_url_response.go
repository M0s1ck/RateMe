package dto

type PresignedGetUrlResponse struct {
	URL string `json:"url" example:"http://localhost:9000/photos/1214a288-1362-408a-918f-5cb9300174ce.jpg?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=minioadmin%2F20250815%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250815T143131Z&X-Amz-Expires=1000&X-Amz-SignedHeaders=host&response-content-disposition=attachment%3B%20filename%3D%221214a280-1162-408a-918f-5cb9300174ce.jpg%22&X-Amz-Signature=97b7e7c5cf44566f2cbfc246eeb26493267e8ff4afc184532d8b3f4af0b5e142"`
}

type PresignedUploadUrlResponse struct {
	URL string `json:"url" example:"http://localhost:9000/photos/1214a280-1162-408a-918f-5cb9300174ce.jpg?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=minioadmin%2F20250815%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250815T140417Z&X-Amz-Expires=1000&X-Amz-SignedHeaders=host&X-Amz-Signature=dee82423f46583c7027b704d486620dc601766fd198887d60345cc3ee9872549"`
}
