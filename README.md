# RateLimitStratsWithRedis

This project demonstrates three common rate limiting algorithms implemented with Redis:

1. Fixed Window Counter
Description:
Limits requests in a fixed time window
If the limit is reached, further requests are denied until the window resets.

Pros:
Simple and fast.
Suitable for steady traffic.
Cons:
Can cause burst traffic at window boundaries ("thundering herd" problem).

2. Sliding Window Log
Description:
Stores timestamps of requests in Redis sorted sets to calculate the number of requests in a moving time frame (e.g., last 60 seconds).

Pros:
More accurate than Fixed Window.
Smooth handling of bursty traffic.
Cons:
Higher memory and processing overhead due to storage of timestamps.

3. Token Bucket
Description:
Tokens are added to a "bucket" at a fixed rate. Each request consumes a token; if no tokens are available, the request is denied.

Pros:
Allows bursts up to bucket capacity.
Smoothens traffic spikes efficiently.

Cons:
Slightly more complex to implement compared to Fixed Window.

![tockenbucketsuccess](https://github.com/user-attachments/assets/1961c845-8405-490f-9214-3f1dd595f137)
![tockenbucketfail](https://github.com/user-attachments/assets/9c465157-e5fa-42fa-8a67-a2dc3d7a1782)
![fixedwindowsuccess](https://github.com/user-attachments/assets/dd2b9d57-60e0-4693-9621-133a82b42484)
![fixedwindowfail](https://github.com/user-attachments/assets/f88ee02b-c6f9-4777-92c7-35ac533599d6)
![slidingwindowsuccess](https://github.com/user-attachments/assets/deae2a40-ccbf-4f6d-a367-d60b7d54e70d)
![slidingwindowfail](https://github.com/user-attachments/assets/d15d7948-5eac-486d-bbe1-ccc7adc1f9c2)
