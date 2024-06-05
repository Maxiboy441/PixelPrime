# Commands

### Build the Docker image
In the ~/Project/Project directory
`
docker build -t my-aspnet-mvc-app .
`

### Start the containers
In the ~/Project/Project directory
`
docker compose up -d
`

### See if container are running  
In the ~/Project/Project directory
`
docker compose ps
`

### Redeploy + Rebuild
On the uwuntu server, as maintainer
In the ~/Project/Project directory
`
cd PixelPrime/schoolproject-3
`
`
git checkout main
`
`
git pull
`
`
cd Project/Project
`
`
docker build -t my-aspnet-mvc-app .
`
`
docker compose restart
`

!!! To not rebuild, dont run "docker build -t my-aspnet-mvc-app ."

### Execute mysql bash command
In the ~/Project/Project directory
`
docker compose exec mysql bash
`
`
mysql -u root -p
`
