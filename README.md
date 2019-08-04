# Beehive
Beehive is a simple cron task scheduler meant to be connected to a Docker host.
It was created for running in a Docker container while connected to its host's docker.sock.
This way it can watch labels for registered containers and run them as per their cron schedule.

Beehive checks every minute if any containers, with the `beehive` labels, are supposed to run that minute.
This means that the highest frequency tasks can run with is 1 minute.

## Starting Beehive
1. Clone the repo
2. Build the image
3. Start via docker run or docker-compose

### docker run
`docker run -d -v /var/run/docker.sock:/var/run/docker.sock beehive`

### docker-compose
```yaml
version: '3.7'
services:
  beehive:
    image: beehive
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
```

## Registering a task
A task is registered by attaching 2 labels to a container:
* `beehive.enable` - task will only be run if set to `true`
* `beehive.cron` - defines the task schedule using [Cronos' extended cron format](https://github.com/HangfireIO/Cronos#cron-format)

### docker create
* Use `docker create` to register a container with a configuration, but without running it:

`docker create -l beehive.enable=true -l "beehive.cron=*/1 * * * *" --name hello-world hello-world`

* Remove and re-add to update the task schedule:

`docker rm hello-world || docker create -l beehive.enable=true -l "beehive.cron=*/5 * * * *" --name hello-world hello-world`

* Remove and re-add to disable the task:

`docker rm hello-world || docker create -l beehive.enable=false -l "beehive.cron=*/1 * * * *" --name hello-world hello-world`

### docker-compose
```yaml
version: '3.7'
services:
  hello-world:
    image: hello-world
    labels:
      - "beehive.enable=true"
      - "beehive.cron=*/1 * * * *"
```

* Register the tasks via up:

`docker-compose up --no-start`

* Update the tasks through changing the docker-compose file and re-running

`docker-compose up --no-start`

* Disable all tasks via down:

`docker-compose down`
