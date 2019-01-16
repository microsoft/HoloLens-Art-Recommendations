from flask import Flask, request, jsonify
import os
import cv2
import numpy as np
import base64
from PIL import Image
from io import BytesIO
import pickle

from feature_extractor import FeatureExtractor
from image_utils import find_k_closest, get_k_nearest_neighbor_image
from data_handler import DataHandler

app = Flask(__name__)

feature_extractor = FeatureExtractor()
with open('data/features_dict.pickle', 'rb') as handle:
    features_dict = pickle.load(handle)

data_handler = DataHandler()

@app.route("/")
def hello():
    return "hello ar art app"

@app.route("/endpoint", methods = ['GET', 'POST'])
def endpoint():
    """
    This endpoing takes a base64 image and returns similar images.
    """
    if request.method == "POST":

        # get the image from the base64 string encoding
        data = request.form['image'].encode("ASCII")
        image = Image.open(BytesIO(base64.b64decode(data)))

        this_vec = feature_extractor(None, image=image)[0].detach().numpy()
        k_closest_list = find_k_closest(this_vec, features_dict)
        image = get_k_nearest_neighbor_image(k_closest_list)
        # image.save("combined.jpg")

        # convert to the correct format to return
        buffered = BytesIO()
        image.save(buffered, format="JPEG")
        img_str = base64.b64encode(buffered.getvalue()).decode("utf-8")

        # construct information list from the k_closest_list
        items = {}
        ordering = []
        for objectid, _ in k_closest_list:
            items[objectid] = data_handler.get_info_from_objectid(objectid)
            ordering.append(objectid)


        data = dict(
            img_str=img_str,
            ordering=ordering,
            items=items
        )

        return jsonify(data)

if __name__ == '__main__':
   app.run(host="0.0.0.0", port=80, debug=True)