
import socket
import threading
from dataset import D

from message import main_message_process

class MySocket:
    
    def __init__(self, host, port) -> None:
        self.addr_archive = {}
        self.message_list = {}
        self.S = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.S.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.S.bind((host, port))
        self.S.listen(10)
        
    def __call__(self):
        self.get_client()
        
    def get_client(self):
        print('Waiting connection...')
        while 1:
            conn, addr = self.S.accept()
            addr = str(addr)
            self.message_list[addr] = ''
            threading.Thread(target=self.receive, args=(conn, addr)).start()
            print('Accept new connection from {}'.format(addr))
            
    def receive(self, conn, addr):
        try:
            while 1:
                if addr not in self.message_list:
                    break
                self.message_list[addr] += conn.recv(1024).decode('utf-8')
                self.message_process(conn, addr)
        except Exception as e:
            print('error {}'.format(e))
            
        if addr in self.message_list:
            del self.message_list[addr]
        if addr in self.addr_archive:
            del self.addr_archive[addr]
        conn.close()
        print('{0} connection close\n'.format(addr))
        
    def message_process(self, conn, addr):
        if '\n' not in self.message_list[addr]:
            return
        x = self.message_list[addr].split('\n')
        for i in range(len(x)-1):
            main_message_process(x[i], conn, addr, S, D)
        self.message_list[addr] = x[-1]
        
    def send(self, message, conn):
        conn.send(bytes(message,"UTF-8"))
        
            
S = MySocket('0.0.0.0', 6666)
