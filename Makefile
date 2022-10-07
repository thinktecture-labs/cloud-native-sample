.PHONY: start stop quickstart logs cleanup

start:
	docker-compose up --build -d

quickstart:
	docker-compose up -d

logs:
	docker-compose logs -f

stop:
	docker-compose down

cleanup:
	docker-compose down --rmi all --volumes --remove-orphans
	
