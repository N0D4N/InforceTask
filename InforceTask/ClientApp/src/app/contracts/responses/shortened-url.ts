import {UserResponse} from "./user-response";

export interface ShortenedUrl {
  id: string;
  destination: string;
  creationDateUnixTimestampInSeconds: number;
  creator: UserResponse;
}
