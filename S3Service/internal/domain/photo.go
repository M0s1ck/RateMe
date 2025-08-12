package domain

type Photo struct {
	Name string
}

func NewPhoto(name string) *Photo {
	return &Photo{Name: name}
}
