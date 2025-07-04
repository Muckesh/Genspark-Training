# Stage 1: Build React App

FROM node:alpine AS builder

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:alpine

# remove default nginx static files
RUN rm -rf /usr/share/nginx/html/*

# Copy build output from previous stage

COPY --from=builder /app/build /usr/share/nginx/html

EXPOSE 80

# start nginx server
CMD [ "nginx", "-g", "daemon off;" ]