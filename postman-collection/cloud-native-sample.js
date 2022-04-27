{
	"info": {
		"_postman_id": "360a336c-4e3e-4c97-bd8e-993eba20ad90",
		"name": "Cloud-Native Sample",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
	},
	"item": [
		{
			"name": "Create Order",
			"request": {
				"method": "POST",
				"header": [],
				"body": {
					"mode": "raw",
					"raw": "{\n    \"customerName\": \"Thorsten Hans\",\n    \"userId\": \"99916ba4-f636-45b7-b5e3-39c74002fda8\",\n    \"positions\": [\n        {\n            \"productId\": \"15616ba4-f636-45b7-b5e3-39c74002fda8\",\n            \"productName\": \"Milk\",\n            \"quantity\": 3\n        }\n    ]\n}",
					"options": {
						"raw": {
							"language": "json"
						}
					}
				},
				"url": {
					"raw": "http://localhost:5000/orders",
					"protocol": "http",
					"host": [
						"localhost"
					],
					"port": "5000",
					"path": [
						"orders"
					]
				}
			},
			"response": []
		}
	]
}