#build stage
FROM golang:alpine AS builder
RUN apk add --no-cache git
WORKDIR /go/src/app
COPY . .
RUN go get -d -v ./...
RUN go build -o /app/shipping-service ./cmd/api.go

#final stage
FROM alpine:latest
RUN apk --no-cache add ca-certificates
WORKDIR /app
COPY ./config.json .
COPY --from=builder /app/shipping-service .
EXPOSE 5000
ENV PORT=5000
CMD ["./shipping-service"]
