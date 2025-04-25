FROM node:18.13.0-alpine

WORKDIR /app

COPY ./package.json .

RUN npm install

COPY . .

RUN npm install -g vite
RUN npm install dayjs

RUN vite build

EXPOSE 80

EXPOSE 443

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173", "--strictPort"]
