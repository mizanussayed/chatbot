FROM  nginx:alpine

#remove the default Nginx configuration
RUN rm /etc/nginx/conf.d/default.conf

#copy our custom configuration
COPY  nginx/chatbot.conf /etc/nginx/conf.d/

#expose port
EXPOSE 80
# start nginx
CMD [ "nginx", "-g", "daemon off;" ]