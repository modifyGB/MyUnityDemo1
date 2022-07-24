import json


def main_message_process(message, conn, addr, socket, dataset):
    print(message)
    message = message.split(' ')
    if len(message) < 3:
        return
    if message[1] == "SAVEALL":
        save_all(message[2], addr, socket, dataset, message[0])
    elif message[1] == "CONNECT":
        connect(message[2], conn, addr, socket, message[0])


#保存全部
def save_all(message, addr, socket, dataset, count):
    if addr not in socket.addr_archive:
        return
    print(socket.addr_archive[addr])
    dataset.L[socket.addr_archive[addr]] = json.loads(message)
    
    dataset.save(socket.addr_archive[addr])
   
    
#链接存档
def connect(message, conn, addr, socket, count):
    if int(message) in socket.addr_archive.values():
        socket.send('no', conn, count)
        return
    socket.addr_archive[addr] = int(message)
    socket.send('yes', conn, count)