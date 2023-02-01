.PHONY: init start stop quickstart logs cleanup 

init:
	docker plugin install grafana/loki-docker-driver:latest --alias loki --grant-all-permissions

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
	
