import json


def main_message_process(message, conn, addr, socket, dataset):
    print(message)
    message = message.split(' ')
    if message[0] == "SAVEALL":
        save_all(message[1], addr, socket, dataset)
    if message[0] == "CONNECT":
        connect(message[1], conn, addr, socket)
    
    if addr in socket.addr_archive:
        dataset.save(socket.addr_archive[addr])
        
        
def save_all(message, addr, socket, dataset):
    if addr not in socket.addr_archive:
        return
    dataset.L[socket.addr_archive[addr]] = json.loads(message)
    
    
def connect(message, conn, addr, socket):
    if int(message) in socket.addr_archive.values():
        socket.send('no', conn)
        return
    socket.addr_archive[addr] = message
    socket.send('yes', conn)