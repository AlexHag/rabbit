# Use the official RabbitMQ image
FROM rabbitmq:3.9-management

# Enable the RabbitMQ stream plugin
RUN rabbitmq-plugins enable --offline rabbitmq_stream