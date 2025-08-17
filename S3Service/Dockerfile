FROM golang:1.24.5-alpine
WORKDIR /app
COPY . .
RUN go mod download
RUN go build -o main ./cmd/app
CMD ["./main"]
