# Augmented Reality Art Museum with the Microsoft Hololens
> This project demonstrates the potential to use augmented reality in art museums. Here we outline an example of a complete application from start to finish. This project makes use of the [The Metropolitan Museum of Art Collection API](https://metmuseum.github.io/). The project consists of two major components, namely the `backend` (data processing and web app endpoing deployment) and `frontend` (Unity application for the Hololens). This README.md file and linked notebooks are used to illustrate the process of replicating this work.

## Local Development Setup
> We use a virtual environment for local development. Here are the steps to make this work on any machine.

```
# create the environment in folder named venv
python3 -m venv venv
# source the evironment
source venv/bin/activate

# if the first time using this
pip install --upgrade pip
pip install -r requirements.txt
ipython kernel install --user --name=arart (now you can start jupyter with `jupyter notebook` and select the arart kernel)

# to deactive the environment
deactivate
```

## Backend
1. iPython Notebook Data Processing
> This section outlines the process of getting the MET data into the correct format for our example application. Note that any art dataset of a similar form. We present the code such that it could be modified for your own custum dataset.
2. Flask Server with Image Search Endpoint
> This section describes the Flask server that hanldes base64 encoded images as inputs and responds with JSON data of the objects.

<servername image similarity endpoint>?base64encoded image

response: {
    similar image names: [list in order of most similar images],
}

3. Deploying the Server as a Docker Application
> We use Docker to create a replicable environment for deployment.
```
# this should be run on a computer with a public IP address
cd main
docker build -t arart .
docker run -d -p 80:80 -v $(pwd)/data:/main/data arart (-d puts this in the background)

# for entering the container
docker run -it --entrypoint /bin/bash -v $(pwd)/data:/main/data arart
```
- Located at http://0.0.0.0:80

## Frontend
> Here we explain the Microsoft Hololens application. We link some important tutorials and then explain where we deviate to create a custom experience with our custom HTTP endpoint.

# TODO

- [ ] verify all data paths are correct in notebooks and server code
- [ ] put data in a place where it can be grabbed if desired



