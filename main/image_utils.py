# file to hold the image utils needed for our work

import numpy as np
import os
from PIL import Image
import matplotlib.pyplot as plt

def np_l2_distance(feat1, feat2):
    # compute the distance between two features
    return np.linalg.norm(feat1-feat2)

def find_k_closest(this_vec, features_dict, k=5):
    closest_and_dists = []
    
    for name, vectors in features_dict.items():
        dist = np_l2_distance(this_vec, vectors)
        
        if len(closest_and_dists) < k:
            closest_and_dists.append((name, dist))
        if dist < closest_and_dists[-1][1]:
            closest_and_dists[-1] = ((name, dist))
        
        closest_and_dists.sort(key = lambda x: x[1])
            
    return closest_and_dists

def get_image_by_objectid(objectid):
    current_dir = os.path.dirname(os.path.abspath(__file__))
    image_filename = os.path.join(current_dir, "data/images/", "{}.jpg".format(objectid))
    image = Image.open(image_filename).convert('RGB')
    return image

def draw_image_by_objectid(objectid):
    image = get_image_by_objectid(objectid)
    plt.imshow(image)
    plt.show()

def get_k_nearest_neighbor_image(k_closest_list, square_size=224):
    # creates and returns a image of all the k closest images aligned horizontally as one image

    combined_image = Image.new('RGB', (square_size*len(k_closest_list), square_size))

    x_offset = 0
    for objectid, _ in k_closest_list:

        image = get_image_by_objectid(objectid)
        image = image.resize((square_size, square_size))
        combined_image.paste(image, (x_offset, 0))
        x_offset += square_size

    return combined_image


