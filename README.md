This is a project for a Trifork internship position, where we use rabbitMQ to send persitent messages between a Producer application and a Consumer application. 
These messages are then either stored in a PostgreSQL database, sent back to RabbitMQ or discarded depending on age of the message. 
We dockerize the applications in order to have multiple instances of the images running.
