
import os
import json
import copy

from struct_data import *


class Dataset:
    
    def __init__(self, save_path) -> None:
        self.L = {}
        self.save_path = save_path
        for i in os.listdir(save_path):
            p = save_path+'/'+i
            with open(p,'r',encoding='utf-8') as f:
                self.L[int(i.split('.')[0])] = json.load(f)
        
    def create(self, num):
        self.L[num] = {
            'GridXZ' : copy.deepcopy(GridXZ),
            'Player' : copy.deepcopy(Player),
        }
        self.save(num)
    
    def save(self, num):
        with open(self.save_path+'/{}.json'.format(num),'w',encoding='utf-8') as f:
            json.dump(self.L[num], f)
            
D = Dataset('data')
