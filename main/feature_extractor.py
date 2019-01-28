import torch
from torch.autograd import Variable
import torchvision
from torchvision import datasets, models, transforms
import glob
from PIL import Image
import cv2
import matplotlib.pyplot as plt
import numpy as np
import os

# this function would unnormalize the normalized image and display it properly
# def imshow(inp, objectID=None):
#     # to verify that our image is good
#     inp = inp.detach().numpy().transpose((1, 2, 0))
#     mean = np.array([0.485, 0.456, 0.406])
#     std = np.array([0.229, 0.224, 0.225])
#     inp = std * inp + mean
#     inp = np.clip(inp, 0, 1)
#     plt.imshow(inp)
#     if objectID is not None:
#         plt.title(objectID)

class FeatureExtractor(torch.nn.Module):
    def __init__(self, image_folder_path=None):
        super(FeatureExtractor, self).__init__()

        # use resnet18 with pretrained weights
        self.model = torchvision.models.resnet18(pretrained=True)
        # put in evaluation mode
        self.model.eval()

        # this should be the absolute path to the image folder directory
        self.image_folder_path = image_folder_path

        # resizes to a square of the correct dimension
        self.loader = transforms.Compose([
            transforms.Resize((224, 224)),
            transforms.ToTensor(),
            transforms.Normalize([0.485, 0.456, 0.406], [0.229, 0.224, 0.225])
        ])

    def normalized_image_from_image(self, image):

        image = self.loader(image).float()
        image = Variable(image, requires_grad=True)
        image = image.unsqueeze(0) # to makes this have a batch dim
        return image.cuda() if torch.cuda.is_available() else image

    def normalized_image_from_filename(self, filename):
        # returns the image tensor given a filename

        # converting to RGB because sometimes they are grayscale and only have 1 channel in that case
        image = Image.open(filename).convert('RGB')
        return self.normalized_image_from_image(image)

    def get_filename_from_objectid(self, objectid):
        # returns the filename given the object id
        return os.path.join(self.image_folder_path, "{}.jpg".format(objectid))

    def get_valid_objectids(self):
        # returns a sorted list of all the object ids
        all_filenames = glob.glob(os.path.join(self.image_folder_path, "*.jpg"))
        objectids = []
        for filename in all_filenames:
            objectid = int(os.path.basename(filename[:filename.rfind('.')]))
            objectids.append(objectid)
        return sorted(objectids)

    def forward(self, objectid, image=None):

        # can call forward with the image data instead of objectid too
        if image is not None:
            image = self.normalized_image_from_image(image)
            return self.model(image)
        else:
            # get the filename
            filename = self.get_filename_from_objectid(objectid)
            # note this image has a batch dimension
            image = self.normalized_image_from_filename(filename)

        return self.model(image)
    