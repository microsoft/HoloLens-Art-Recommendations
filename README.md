# Augmented Reality Art Museum with the Microsoft Hololens
> This project demonstrates the potential to use augmented reality in art museums. Here we outline an example of a complete application from start to finish. This project makes use of the [The Metropolitan Museum of Art Collection API](https://metmuseum.github.io/). The project consists of two major components, namely the `backend` (data processing and web app endpoint deployment) and `frontend` (Unity application for the Hololens). This README.md file and linked notebooks are used to illustrate the process of replicating this work.

## Local Development Setup
> We use a virtual environment for local development. Here are the steps to make this work on any machine.

```
# create the virtual environment in folder named venv
python3 -m venv venv

# source the evironment
source venv/bin/activate

# updgrade pip and install dependencies
pip install --upgrade pip
pip install -r requirements.txt

# install kernel for jupyter notebook
ipython kernel install --user --name=arart

# to deactivate the environment
deactivate
```

We use `virtualenv` for local testing but `Docker` for deployment (which is described later). The code is written in Python, and all dependencies are listed in the [requirements.txt](main/requirements.txt) file.

## Backend
1. iPython Notebook Data Processing
> This section outlines the process of getting the MET data into the correct format for our example application. Note that any art dataset of a similar form. We present the code such that it could be modified for your own custom dataset.

Follow these notebooks in order to format the data properly. All data should end up in a nicely formatted `data` directory, which is located in the `main` folder. [`data`](main/data) should have an `images` folder with <objectid>.jpg names and a .csv file with data for each object.

- [CreateDatasetFromMetAPI.ipynb](notebooks/CreateDatasetFromMetAPI.ipynb)
- [SaveImagesFromCsvURLs.ipynb](notebooks/SaveImagesFromCsvURLs.ipynb)
- [CreateFeaturesDictionary.ipynb](notebooks/CreateFeaturesDictionary.ipynb)
    - This notebook uses ResNet18 with weights pretrained on ImageNet. Removing the last layer allows us to extract the feature vector, which encodes information pertaining to the class of the item. This allows us to use L2 distance between feature vectors to do image similarity comparisons.
- [FindNearestNeighbors.ipynb](notebooks/FindNearestNeighbors.ipynb)

2. Flask Server with Image Search Endpoint
> This section describes the Flask server that hanldes base64 encoded images as inputs and responds with JSON data of the objects.

<servername image similarity endpoint>?base64encoded image

response: {
    similar image names: [list in order of most similar images],
}

```
data = {
    "img_str": base64 string of the new images,
    "ordering": [objectid1, objectid2, etc.],
    "items_info": [
        {
            "objectid": objectid1,
            "information": [
                {"title": title information, "description": des 1},
                {"title": title information, "description": des 2}
            ],
        },
        {
            "objectid": objectid2,
            "information": [
                {"title": title information, "description": des 1},
                {"title": title information, "description": des 2}
            ],
        }
    ]
}
```

3. Deploying the Server as a Docker Application
> We use Docker to create a replicable environment for deployment.
```
# this should be run on a computer with a public IP address
cd main
docker build -t arart .
docker run -d -p 80:80 -v $(pwd)/data:/main/data arart (-d puts this in the background)

# for entering the container
docker run -it --entrypoint /bin/bash -p 80:80 -v $(pwd)/data:/main/data arart
```
- Located at http://0.0.0.0:80

## Frontend
> Here we explain the Microsoft Hololens application. We link some important tutorials and then explain where we deviate to create a custom experience with our custom HTTP endpoint.

1. Choose what to display
    - Title (always)
    - when available:
        - department
        - Culture
        - artistRole
        - objectEndDate
        - medium
        - creditLine
        - geographyType
        - classification


# TODO

- [ ] verify all data paths are correct in notebooks and server code
- [ ] put data in a place where it can be grabbed if desired
- [ ] accidentally committed private key in VisionManager.cs, so fix this in git is possible.
- [ ] mount the python files instead of creating the image with them inside



