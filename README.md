# AR Art for The Metropolitan Museum of Art Collection

## API description
- https://metmuseum.github.io/

## Using a virtual environment
create the environment in folder named venv
```
python3 -m venv venv
```

source the evironment
```
source venv/bin/activate
```

if for the first time
```
pip install --upgrade pip
pip install -r requirements.txt
ipython kernel install --user --name=arart (now you can start jupyter with `jupyter notebook` and select the arart kernel)
```

to deactivate the environment
```
deactivate
```

TODO(ethanweber)
## Image Similarity Endpoint

<servername image similarity endpoint>?base64encoded image

response: {
    similar image names: [list in order of most similar images],
}