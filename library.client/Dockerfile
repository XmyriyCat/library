FROM node:18 AS builder
WORKDIR /app

ENV REACT_APP_API_BASE_URL=http://localhost:8080/api

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build


FROM nginx:alpine
RUN rm -rf /usr/share/nginx/html/*

COPY --from=builder /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
