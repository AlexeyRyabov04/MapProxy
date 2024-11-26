# Use the official Node.js image as a base image
FROM node:18-alpine

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and package-lock.json (or yarn.lock) first
# This allows us to leverage Docker's caching mechanism
COPY package*.json ./

# Install production dependencies only
RUN npm install

# Copy the rest of the application code into the container
COPY . .

# Expose the port the application will run on
EXPOSE ${PORT}

# Command to run the application using npm
CMD ["npm", "start"]
