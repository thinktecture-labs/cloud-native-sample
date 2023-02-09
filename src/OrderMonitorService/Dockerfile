#build stage
FROM golang:alpine AS builder
RUN apk add --no-cache git
WORKDIR /go/src/app
COPY . .
RUN go get -d -v ./...
RUN go build -o /app/ordermonitor-service ./cmd

#final stage
FROM alpine:latest
RUN apk --no-cache add ca-certificates
RUN addgroup -S appgroup && adduser -S appuser -G appgroup -h /app
USER appuser
WORKDIR /app
COPY --chown=appuser:appgroup ./config.json .
COPY --chown=appuser:appgroup --from=builder /app/ordermonitor-service .
EXPOSE 5000
ENV PORT=5000
CMD ["./ordermonitor-service"]
