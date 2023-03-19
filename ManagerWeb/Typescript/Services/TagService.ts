import { TagApiInterface } from "../ApiClient/Main";

export default class TagService {
    private tagApi: TagApiInterface;

    constructor(tagApi: TagApiInterface) {
        this.tagApi = tagApi;
    }

    public async getAllUsedTags() {
        const tags = await this.tagApi.tagsAllUsedGet();
        return tags;
    }
}