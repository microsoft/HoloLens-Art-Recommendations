FROM ubuntu:latest

RUN apt-get update \
&& apt-get install -y python3-pip python3-dev \
&& cd /usr/local/bin \
&& ln -s /usr/bin/python3 python \
&& pip3 install --upgrade pip \
&& apt-get install -y libsm6 libxrender1 libfontconfig1 \
&& apt update && apt install -y libsm6 libxext6

COPY . /main
WORKDIR /main

RUN pip3 install -r requirements.txt

ENTRYPOINT ["python"]
CMD ["server.py"]