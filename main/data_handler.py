import os
import copy
import numpy as np
import pandas as pd

class DataHandler(object):
    """
    Handles processing information from the CSV.
    """
    def __init__(self):
        
        self.current_dir = os.path.dirname(os.path.abspath(__file__))
        self.df = pd.read_csv("data/metdata.csv", skipinitialspace=True)

    def get_info_from_objectid(self, objectid):

        row_index = self.df['objectID'].tolist().index(objectid)
        # this gets the data associated with a given object
        original_info = self.df.iloc[row_index,:].to_dict()

        info = {}
        for key in original_info.keys():
            # hold the original val and the new val
            original_val = original_info[key]
            new_val = copy.copy(original_val)

            # if 'nan' then continue
            try:
                if np.isnan(original_val):
                    continue
            except:
                pass

            if type(original_val) == np.bool_:
                new_val = 1.0 if original_val else 0.0
            elif type(original_val) == str:
                pass
            elif type(original_val) == np.int64:
                new_val = int(original_val)
            elif type(original_val) == np.float64:
                new_val = float(original_val)
            
            info[key] = new_val
        
        return info